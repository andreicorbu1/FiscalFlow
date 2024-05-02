import {Component, OnInit} from '@angular/core';
import {UserService} from "../user/user.service";
import {Account} from "../shared/models/account/account";
import {AccountService} from "../account/account.service";
import {Transaction} from "../shared/models/transaction/transaction";
import {TransactionService} from "../transaction/transaction.service";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  accounts: Account[] | undefined;
  displayedColumns: string[] = ['CreatedOnUtc','Account', 'Payee', 'Value'];

  // @ts-ignore
  transactions: Transaction[];
  constructor(public userService: UserService, private accountService: AccountService, private transactionService: TransactionService) {
  }

  ngOnInit(): void {
    this.accountService.getAllAccounts().subscribe((accounts) => {
        this.accounts = accounts;
        console.log(this.accounts[0]?.transactions);
      }
    );
    this.transactionService.getLastTransactions(10).subscribe((transactions) => {
      this.transactions = transactions;
      console.log(transactions);
    })
  }
}
