import {Component, OnInit} from '@angular/core';
import {UserService} from "../user/user.service";
import {Account} from "../shared/models/account/account";
import {AccountService} from "../account/account.service";
import {Transaction} from "../shared/models/transaction/transaction";
import {TransactionService} from "../transaction/transaction.service";
import {MatDialog} from "@angular/material/dialog";
import {AddTransactionComponent} from "../transaction/add-transaction/add-transaction.component";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  // @ts-ignore
  accounts: Account[];
  displayedColumns: string[] = ['CreatedOnUtc','Account', 'Payee', 'Value', 'Actions'];

  // @ts-ignore
  transactions: Transaction[];
  constructor(public userService: UserService, private dialog: MatDialog, private accountService: AccountService, private transactionService: TransactionService) {
  }


  ngOnInit(): void {
    this.accountService.getAllAccounts().subscribe((accounts) => {
        this.accounts = accounts;
      }
    );
    this.transactionService.getLastTransactions(10).subscribe((transactions) => {
      this.transactions = transactions;
    })
  }

  refreshAccounts($event: boolean) {
    if($event) {
      this.ngOnInit();
    }
  }
}
