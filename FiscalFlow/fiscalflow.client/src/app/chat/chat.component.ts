import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { OpenaiService } from '../openai.service';
import { Transaction } from '../shared/models/transaction/transaction';

export interface MessageDto {
  role: string;
  content: string;
}

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss'],
})
export class ChatComponent {
  transactions: Transaction[] = [];
  messages: MessageDto[] = [];
  message: string = '';
  isLoading: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private openAiService: OpenaiService
  ) {
    if (data && data.transactions) {
      this.transactions = data.transactions;
    }
  }

  sendMessage() {
    this.isLoading = true;
    this.messages.push({ role: 'user', content: this.message });
    const localMessage = this.message;
    this.message = '';
    if (this.messages.length == 1) {
      this.openAiService
        .sendFirstMessageChat(this.transactions, localMessage)
        .subscribe((response) => {
          this.messages.push({
            role: 'assistant',
            //@ts-ignore
            content: response.choices[0].message.content,
          });
          this.isLoading = false;
        });
    } else {
      this.openAiService.sendMessage(this.messages).subscribe({
        next: (response) => {
          this.messages.push({
            role: 'assistant',
            //@ts-ignore
            content: response.choices[0].message.content,
          });
          this.isLoading = false;
        },
        error: (error) => {
          this.isLoading = false;
        },
      });
    }
  }
}
