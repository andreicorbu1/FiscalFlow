import {Component, Input} from '@angular/core';
import {Account} from "../../shared/models/account/account";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {AddTransactionComponent} from "../../transaction/add-transaction/add-transaction.component";
import {TransactionType} from "../../shared/models/transaction/enums/transactionType";

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent {
@Input() account: Account | undefined;
  constructor(private dialog: MatDialog) {
    console.log(this.account?.transactions);
  }
  openAddTransactionForm() {
    const dialogRef = this.dialog.open(AddTransactionComponent, {
      data: this.account,
    });
    dialogRef.afterClosed().subscribe( {
      next: (dialog: MatDialogRef<AddTransactionComponent>) => {
        window.location.reload();
      }
    })
  }

  protected readonly TransactionType = TransactionType;
}
