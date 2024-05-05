import {Component, Input, OnInit} from '@angular/core';
import {Account} from "../../shared/models/account/account";
import {AccountService} from "../account.service";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-account-detail',
  templateUrl: './account-detail.component.html',
  styleUrls: ['./account-detail.component.scss']
})
export class AccountDetailComponent implements OnInit{
  // @ts-ignore
  account: Account = null;
  constructor(private accountService: AccountService, private route: ActivatedRoute) {
  }
  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      // @ts-ignore
      const accountId: string = params.get('id');
      this.accountService.getAccountId(accountId).subscribe({
        next: (account: Account) => {
          console.log(account);
          this.account = account;
        },
        error: _ => {
          console.log(_);
        }
      });
    });
  }
  refreshAccount($event: boolean) {
    if($event) {
      this.ngOnInit();
    }
  }
}
