import {Component, OnInit} from '@angular/core';
import {UserService} from "../user/user.service";
import {Account} from "../shared/models/account/account";
import {AccountService} from "../account/account.service";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  accounts: Account[] | undefined;

  constructor(public userService: UserService, private accountService: AccountService) {
  }

  ngOnInit(): void {
    this.accountService.getAllAccounts().subscribe((accounts) => {
        this.accounts = accounts;
      }
    );
  }
}
