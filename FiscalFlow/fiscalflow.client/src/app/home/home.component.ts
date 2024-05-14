import { Component, OnInit } from '@angular/core';
import { UserService } from '../user/user.service';
import { Account } from '../shared/models/account/account';
import { AccountService } from '../account/account.service';
import { Transaction } from '../shared/models/transaction/transaction';
import { TransactionService } from '../transaction/transaction.service';
import { MatDialog } from '@angular/material/dialog';
import { AddTransactionComponent } from '../transaction/add-transaction/add-transaction.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  // @ts-ignore
  accounts: Account[];

  // @ts-ignore
  transactions: Transaction[];
  constructor(
    public userService: UserService,
    private router: Router,
    private accountService: AccountService,
    private transactionService: TransactionService
  ) {}

  ngOnInit(): void {
    this.accountService.getAllAccounts().subscribe((accounts) => {
      this.accounts = accounts;
    });
    this.transactionService
      .getLastTransactions(10)
      .subscribe((transactions) => {
        this.transactions = transactions;
      });
  }

  refreshAccounts($event: boolean) {
    if ($event) {
      this.ngOnInit();
    }
  }
  navigateToRegister() {
    this.router.navigate(['/user/register']); // Adjust the path to your registration page
  }
}
