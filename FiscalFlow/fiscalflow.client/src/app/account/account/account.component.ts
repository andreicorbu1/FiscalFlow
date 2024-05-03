import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Account} from "../../shared/models/account/account";
import {MatDialog} from "@angular/material/dialog";
import {AddTransactionComponent} from "../../transaction/add-transaction/add-transaction.component";

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent {
  @Output() newTransactionEvent = new EventEmitter();
  @Input() account: Account | undefined;
  constructor(private dialog: MatDialog) {
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
        this.newTransactionEvent.emit(true);
      }
    })
  }
}
