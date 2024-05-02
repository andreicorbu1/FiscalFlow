import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {AddTransaction} from "../shared/models/transaction/addTransaction";
import {environment} from "../../environments/environment.development";
import {Transaction} from "../shared/models/transaction/transaction";

@Injectable({
  providedIn: 'root'
})
export class TransactionService {

  constructor(private httpClient: HttpClient) { }

  addTransaction(transaction: AddTransaction) {
    return this.httpClient.post(`${environment.appUrl}/api/v1/transaction`, transaction);
  }

  getLastTransactions(transactions: number) {
    return this.httpClient.get<Transaction[]>(`${environment.appUrl}/api/v1/account/me/account/last-${transactions}-transactions`);
  }

}
