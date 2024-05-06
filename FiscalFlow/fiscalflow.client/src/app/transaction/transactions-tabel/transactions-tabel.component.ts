import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Transaction} from "../../shared/models/transaction/transaction";
import {AddTransactionComponent} from "../add-transaction/add-transaction.component";
import {MatDialog} from "@angular/material/dialog";
import {TransactionService} from "../transaction.service";
import {MatTableModule} from "@angular/material/table";
import {DatePipe} from "@angular/common";
import {MatButtonModule} from "@angular/material/button";
import {MatTooltipModule} from "@angular/material/tooltip";

@Component({
  selector: 'app-transactions-tabel',
  templateUrl: './transactions-tabel.component.html',
  standalone: true,
  imports: [
    MatTableModule,
    DatePipe,
    MatButtonModule,
    MatTooltipModule
  ],
  styleUrls: ['./transactions-tabel.component.scss']
})
export class TransactionsTabelComponent {
  // @ts-ignore
  @Input() transactions: Transaction[];
  @Output() modifiedTransactionEvent = new EventEmitter();
  displayedColumns: string[] = ['CreatedOnUtc','Account', 'Payee', 'Value', 'Actions'];
  constructor(private dialog: MatDialog, private transactionService: TransactionService) {
  }
  onEditTransaction(transaction: Transaction) {
    const dialogRef = this.dialog.open(AddTransactionComponent, {
      data: {
        account: {
          // @ts-ignore
          name: transaction.account,
          currency: transaction.currency,
        },
        transaction: transaction,
      }
    });
    dialogRef.afterClosed().subscribe( result => {
      if(result === true) {
        this.modifiedTransactionEvent.emit(true);
      }
    });
  }
  onDeleteTransaction(transaction: Transaction) {
    this.transactionService.deleteTransaction(transaction.id).subscribe({
      next: data => {
        this.modifiedTransactionEvent.emit(true);
      },
      error: error => {
        console.log(error);
      }
    });
  }
}
