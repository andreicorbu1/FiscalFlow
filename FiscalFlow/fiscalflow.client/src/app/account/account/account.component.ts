import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Account} from "../../shared/models/account/account";
import {MatDialog} from "@angular/material/dialog";
import {AddTransactionComponent} from "../../transaction/add-transaction/add-transaction.component";
import {Router} from "@angular/router";

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent {
  @Output() newTransactionEvent = new EventEmitter();
  // @ts-ignore
  @Input() account: Account | null;
  constructor(private dialog: MatDialog, private router: Router) {
  }
  openAddTransactionForm(event: MouseEvent) {
    event.stopPropagation();
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

  navigateToDetails() {
    // @ts-ignore
    this.router.navigate(['/account/details', this.account?.id]);
  }
}
