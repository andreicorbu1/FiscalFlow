import {
  AfterViewInit,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild
} from '@angular/core';
import {Transaction} from "../../shared/models/transaction/transaction";
import {AddTransactionComponent} from "../add-transaction/add-transaction.component";
import {MatDialog} from "@angular/material/dialog";
import {TransactionService} from "../transaction.service";
import {MatTableModule} from "@angular/material/table";
import {AsyncPipe, DatePipe, NgIf, SlicePipe} from "@angular/common";
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
    SlicePipe,
    AsyncPipe
  ],
  styleUrls: ['./transactions-tabel.component.scss']
})
export class TransactionsTabelComponent implements OnChanges{
  @Input() transactions: Transaction[];
  filteredTransactions: Transaction[];
  @Output() modifiedTransactionEvent = new EventEmitter();
  displayedColumns: string[] = ['CreatedOnUtc', 'Account', 'Payee', 'Value', 'Actions'];
  startDate: Date;
  endDate: Date;
  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;


  constructor(private dialog: MatDialog, private transactionService: TransactionService) {
  }

  ngOnChanges(changes: SimpleChanges): void {
      this.filteredTransactions = [...this.transactions];
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
    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
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
    this.filteredTransactions = this.filteredTransactions.filter(transaction => {
      const transactionDate = new Date(transaction.createdOnUtc);
      if (this.startDate && this.endDate) {
        return transactionDate >= this.startDate && transactionDate <= this.endDate;
      }
      if (!this.startDate && this.endDate) {
        return transactionDate <= this.endDate;
      }
      if (this.startDate && !this.endDate) {
        return transactionDate >= this.startDate;
      }
      return true;
    });
  }


  resetDateFilter() {
    // @ts-ignore
    this.startDate = '';
    // @ts-ignore
    this.endDate = '';
    this.filteredTransactions = this.transactions;
  }
}
