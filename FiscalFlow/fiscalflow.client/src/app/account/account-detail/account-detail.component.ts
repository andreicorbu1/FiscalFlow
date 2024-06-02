import { Component, Input, OnInit } from '@angular/core';
import { Account } from '../../shared/models/account/account';
import { AccountService } from '../account.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AddTransactionComponent } from '../../transaction/add-transaction/add-transaction.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-account-detail',
  templateUrl: './account-detail.component.html',
  styleUrls: ['./account-detail.component.scss'],
})
export class AccountDetailComponent implements OnInit {
  // @ts-ignore
  account: Account = null;
  constructor(
    private accountService: AccountService,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private router: Router
  ) {}
  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      // @ts-ignore
      const accountId: string = params.get('id');
      this.accountService.getAccountId(accountId).subscribe({
        next: (account: Account) => {
          this.account = account;
        },
        error: (_) => {
          console.log(_);
        },
      });
    });
  }
  refreshAccount($event: boolean) {
    if ($event) {
      this.ngOnInit();
    }
  }
  openAddTransactionForm() {
    const dialogRef = this.dialog.open(AddTransactionComponent, {
      data: {
        account: this.account,
        transaction: null,
      },
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result === true) {
        this.ngOnInit();
      }
    });
  }

  downloadCsv() {
    console.log('here 1');
    this.accountService
      .getTransactionsFromAccountAsCsv(this.account.id)
      .subscribe(
        (response: Blob) => {
          const blob = new Blob([response], { type: 'text/csv' });
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url;
          a.download = 'transactions.csv';
          a.click();
          window.URL.revokeObjectURL(url);
        },
        (error) => {
          console.log(error);
        }
      );
  }

  importTransactionsFromCSV() {
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = '.csv';
    input.onchange = (event: any) => {
      const file = event.target.files[0];
      if (file) {
        const formData = new FormData();
        formData.append('file', file);

        const accountId = this.account.id;

        this.accountService.importTransactions(accountId, formData).subscribe({
          next: () => {
            this.refreshAccount(true);
          },
          error: (error) => {
            console.error('Import failed', error);
          },
        });
      }
    };
    input.click();
  }

  deleteAccount() {
    this.accountService.deleteAccount(this.account.id).subscribe({
      next: () => {
        this.router.navigateByUrl('/');      
      },
      error: (error) => {
        console.error('Delete failed', error);
      },
    });
  }

  editAccount() {
    throw new Error('Method not implemented.');
  }
}
