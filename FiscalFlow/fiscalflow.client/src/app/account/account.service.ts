import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { CreateAccountRequest } from '../shared/models/account/createAccount';
import { environment } from '../../environments/environment.development';
import { Account } from '../shared/models/account/account';
import { UpdateAccount } from '../shared/models/account/updateAccount';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  constructor(private httpClient: HttpClient, private router: Router) {}

  addAccount(account: CreateAccountRequest) {
    return this.httpClient.post(
      `${environment.appUrl}/api/v1/account`,
      account
    );
  }

  getAllAccounts() {
    return this.httpClient.get<Account[]>(
      `${environment.appUrl}/api/v1/Account/me/accounts`
    );
  }

  getAccountId(accountId: string) {
    return this.httpClient.get<Account>(
      `${environment.appUrl}/api/v1/Account/me/account/${accountId}`
    );
  }

  getCategoryExpenses() {
    return this.httpClient.get<any>(
      `${environment.appUrl}/api/v1/Account/me/category-expenses`
    );
  }

  getTransactionsFromAccountAsCsv(accountId: string) {
    return this.httpClient.get(
      `${environment.appUrl}/api/v1/Account/me/csv/export/${accountId}`,
      {
        responseType: 'blob',
      }
    );
  }

  importTransactions(accountId: string, formData: FormData) {
    return this.httpClient.post(
      `${environment.appUrl}/api/v1/Account/me/csv/import/${accountId}`,
      formData
    );
  }

  deleteAccount(accountId: string) {
    return this.httpClient.delete(
      `${environment.appUrl}/api/v1/Account/me/delete/${accountId}`
    );
  }

  updateAccount(accountId: string, updateAccount: UpdateAccount) {
    return this.httpClient.put(
      `${environment.appUrl}/api/v1/Account/me/update/account=${accountId}`,
      updateAccount
    );
  }
}
