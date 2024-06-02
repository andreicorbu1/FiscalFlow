import {
  AfterViewInit,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { Transaction } from '../../shared/models/transaction/transaction';
import { AddTransactionComponent } from '../add-transaction/add-transaction.component';
import { MatDialog } from '@angular/material/dialog';
import { TransactionService } from '../transaction.service';
import { MatTableModule } from '@angular/material/table';
import { AsyncPipe, DatePipe, NgFor, NgIf, SlicePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatOptionModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';

interface Category {
  value: number;
  viewValue: string;
}

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
    NgFor,
    MatOptionModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    FormsModule,
    MatPaginatorModule,
    SlicePipe,
    AsyncPipe,
  ],
  styleUrls: ['./transactions-tabel.component.scss'],
})
export class TransactionsTabelComponent implements OnChanges {
  @Input() transactions: Transaction[];
  filteredTransactions: Transaction[];
  @Output() modifiedTransactionEvent = new EventEmitter();
  displayedColumns: string[] = [
    'CreatedOnUtc',
    'Account',
    'Payee',
    'Value',
    'Actions',
  ];
  selectedCategories: number[] = [];
  categories: Category[] = [
    { value: 0, viewValue: 'Food and Drinks' },
    { value: 1, viewValue: 'Shopping' },
    { value: 2, viewValue: 'House' },
    { value: 3, viewValue: 'Transportation' },
    { value: 4, viewValue: 'Vehicle' },
    { value: 5, viewValue: 'Life and Entertainment' },
    { value: 6, viewValue: 'Communication and Pc' },
    { value: 7, viewValue: 'Financial Expenses' },
    { value: 8, viewValue: 'Investments' },
    { value: 9, viewValue: 'Income' },
    { value: 10, viewValue: 'Other' },
  ];
  startDate: Date;
  endDate: Date;
  @ViewChild(MatPaginator, { static: true }) pag!: MatPaginator;

  constructor(
    private dialog: MatDialog,
    private transactionService: TransactionService
  ) {}

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
      },
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result === true) {
        this.modifiedTransactionEvent.emit(true);
      }
    });
  }

  onDeleteTransaction(transaction: Transaction) {
    this.transactionService.deleteTransaction(transaction.id).subscribe({
      next: (data) => {
        this.modifiedTransactionEvent.emit(true);
      },
      error: (error) => {
        console.log(error);
      },
    });
  }

  applyDateFilter() {
    this.filteredTransactions = this.transactions.filter(
      (transaction) => {
        const transactionDate = new Date(transaction.createdOnUtc);
        if (this.startDate && this.endDate) {
          return (
            transactionDate >= this.startDate && transactionDate <= this.endDate
          );
        }
        if (!this.startDate && this.endDate) {
          return transactionDate <= this.endDate;
        }
        if (this.startDate && !this.endDate) {
          return transactionDate >= this.startDate;
        }
        if (this.selectedCategories.length > 0) {
          return this.selectedCategories.includes(transaction.category);
        }
        return true;
      }
    );
  }

  resetDateFilter() {
    // @ts-ignore
    this.startDate = '';
    // @ts-ignore
    this.endDate = '';
    this.filteredTransactions = this.transactions;
    this.selectedCategories = [];
  }
}
