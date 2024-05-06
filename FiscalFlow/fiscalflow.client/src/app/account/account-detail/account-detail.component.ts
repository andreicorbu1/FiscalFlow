import {Component, Input, OnInit} from '@angular/core';
import {Account} from "../../shared/models/account/account";
import {AccountService} from "../account.service";
import {ActivatedRoute} from "@angular/router";
import {AddTransactionComponent} from "../../transaction/add-transaction/add-transaction.component";
import {MatDialog} from "@angular/material/dialog";

@Component({
  selector: 'app-account-detail',
  templateUrl: './account-detail.component.html',
  styleUrls: ['./account-detail.component.scss']
})
export class AccountDetailComponent implements OnInit{
  // @ts-ignore
  account: Account = null;
  constructor(private accountService: AccountService, private route: ActivatedRoute,
              private dialog: MatDialog) {
  }
  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      // @ts-ignore
      const accountId: string = params.get('id');
      this.accountService.getAccountId(accountId).subscribe({
        next: (account: Account) => {
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
  openAddTransactionForm() {
    const dialogRef = this.dialog.open(AddTransactionComponent, {
      data: {
        account: this.account,
        transaction: null,
      }
    });
    dialogRef.afterClosed().subscribe( result => {
      if(result === true) {
        this.ngOnInit();
      }
    });
  }
}
