import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild} from '@angular/core';
import {Transaction} from "../../shared/models/transaction/transaction";
import {AddTransactionComponent} from "../add-transaction/add-transaction.component";
import {MatDialog} from "@angular/material/dialog";
import {TransactionService} from "../transaction.service";
import {MatTableModule} from "@angular/material/table";
import {DatePipe, NgIf, SlicePipe} from "@angular/common";
import {MatButtonModule} from "@angular/material/button";
import {MatTooltipModule} from "@angular/material/tooltip";
import {MatDatepickerModule} from "@angular/material/datepicker";
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatInputModule} from "@angular/material/input";
import {FormsModule} from "@angular/forms";
import {MatPaginator, MatPaginatorModule} from "@angular/material/paginator";

@Component({
  selector: 'app-transactions-tabel',
  templateUrl: './transactions-tabel.component.html',
  standalone: true,
  imports: [
    MatTableModule,
    DatePipe,
    MatButtonModule,
    MatTooltipModule,
    NgIf,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatPaginatorModule,
    SlicePipe
  ],
  styleUrls: ['./transactions-tabel.component.scss']
})
export class TransactionsTabelComponent implements OnChanges {
  // @ts-ignore
  @Input() transactions: Transaction[];
  // @ts-ignore
  filteredTransactions: Transaction[];
  @Output() modifiedTransactionEvent = new EventEmitter();
  displayedColumns: string[] = ['CreatedOnUtc', 'Account', 'Payee', 'Value', 'Actions'];
  // @ts-ignore
  startDate: Date;
  // @ts-ignore
  endDate: Date;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;



  constructor(private dialog: MatDialog, private transactionService: TransactionService) {
  }

  ngOnChanges() {
    if (this.transactions && this.transactions.length > 0) {
      this.filteredTransactions = [...this.transactions];
    } else {
      this.filteredTransactions = [];
    }
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
  applyDateFilter() {
    if (this.startDate && this.endDate) {
      this.filteredTransactions = this.transactions.filter(transaction => {
        const transactionDate = new Date(transaction.createdOnUtc);
        return transactionDate >= this.startDate && transactionDate <= this.endDate;
      });
    }
  }

  resetDateFilter() {
    // @ts-ignore
    this.startDate = '';
    // @ts-ignore
    this.endDate = '';
    this.filteredTransactions = this.transactions;
  }
}
