import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";
import {CreateAccountRequest} from "../shared/models/account/createAccount";
import {environment} from "../../environments/environment.development";
import {Account} from "../shared/models/account/account";

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private httpClient: HttpClient, private router: Router) { }

  addAccount(account: CreateAccountRequest) {
    return this.httpClient.post(`${environment.appUrl}/api/v1/account`, account);
  }

  getAllAccounts() {
    return this.httpClient.get<Account[]>(`${environment.appUrl}/api/v1/Account/me/accounts`);
  }

  getAccountId(accountId: string) {
    return this.httpClient.get<Account>(`${environment.appUrl}/api/v1/Account/me/account/${accountId}`);
  }
}
