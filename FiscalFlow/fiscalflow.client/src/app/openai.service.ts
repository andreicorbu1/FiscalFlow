import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Transaction } from './shared/models/transaction/transaction';
import { MessageDto } from './chat/chat.component';

export interface GptHistory {
  model: string;
  messages: MessageDto[];
}

@Injectable({
  providedIn: 'root',
})
export class OpenaiService {
  headers = new HttpHeaders({
    'Content-Type': 'application/json',
    Authorization: `Bearer ${environment.openaiKey}`,
  });
  constructor(private httpClient: HttpClient) {}
  getSuggestedCategories(payee: string, description: string) {
    const message: string = `{
      "model": "gpt-3.5-turbo",
      "messages": [
          {
              "role": "user",
              "content": "Based on this Payee: ${payee}; and this description: ${description}; select one of the following categories that you think is the best suited: FoodAndDrinks, Shopping, House, Transportation, Vehicle, LifeAndEntertainment, Finance, HealthAndPersonalCare, Income, Others? Answer only with the category and choose only a category from the ones i gave you!"
          }
      ]
  }`;
    const headers = this.headers;
    return this.httpClient.post(
      'https://api.openai.com/v1/chat/completions',
      message,
      { headers }
    );
  }

  sendMessage(oldMessages: MessageDto[]) {
    const headers = this.headers;
    let messages: GptHistory = {
      model: 'gpt-3.5-turbo',
      messages: oldMessages,
    };
    return this.httpClient.post(
      'https://api.openai.com/v1/chat/completions',
      messages,
      { headers }
    );
  }

  sendFirstMessageChat(transactions: Transaction[], message: string) {
    const headers = this.headers;
    const messages: string = `{
      "model": "gpt-3.5-turbo",
      "messages": [
          {
              "role": "system",
              "content": "From now on you will act as a personal finance assistant. You are a personal finance assistant on a budget tracking app. Only answer to question that have to do with finances."
          },
          {
            "role": "user",
            "content": "${message}"
          }
      ]
    }`;
    return this.httpClient.post(
      'https://api.openai.com/v1/chat/completions',
      messages,
      { headers }
    );
  }
}
