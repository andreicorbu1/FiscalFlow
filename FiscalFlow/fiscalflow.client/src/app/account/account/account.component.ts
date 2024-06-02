import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Account } from '../../shared/models/account/account';
import { MatDialog } from '@angular/material/dialog';
import { AddTransactionComponent } from '../../transaction/add-transaction/add-transaction.component';
import { Router } from '@angular/router';
import { Transaction } from 'src/app/shared/models/transaction/transaction';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss'],
})
export class AccountComponent implements OnInit {
  @Output() newTransactionEvent = new EventEmitter();
  // @ts-ignore
  @Input() account: Account | null;
  transactions: Transaction[];
  constructor(private dialog: MatDialog, private router: Router) {}

  ngOnInit(): void {
    this.transactions = this.account?.transactions.slice(0, 3) ?? [];
  }

  openAddTransactionForm(event: MouseEvent) {
    event.stopPropagation();
    const dialogRef = this.dialog.open(AddTransactionComponent, {
      data: {
        account: this.account,
        transaction: null,
      },
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result === true) {
        this.newTransactionEvent.emit(true);
      }
    });
  }

  navigateToDetails() {
    // @ts-ignore
    this.router.navigate(['/account/details', this.account?.id]);
  }
}
