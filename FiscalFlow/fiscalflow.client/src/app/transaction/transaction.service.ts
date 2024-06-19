import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AddTransaction } from '../shared/models/transaction/addTransaction';
import { environment } from '../../environments/environment.development';
import { Transaction } from '../shared/models/transaction/transaction';
import { UpdateTransaction } from '../shared/models/transaction/updateTransaction';
import { Subscription } from '../shared/models/transaction/subscription';

@Injectable({
  providedIn: 'root',
})
export class TransactionService {
  constructor(private httpClient: HttpClient) {}

  addTransaction(transaction: AddTransaction) {
    return this.httpClient.post(
      `${environment.appUrl}/api/v1/transaction`,
      transaction
    );
  }

  deleteTransaction(id: string) {
    return this.httpClient.delete(
      `${environment.appUrl}/api/v1/transaction/me/delete/${id}`
    );
  }

  getLastTransactions(transactions: number) {
    return this.httpClient.get<Transaction[]>(
      `${environment.appUrl}/api/v1/account/me/account/last-${transactions}-transactions`
    );
  }

  deleteSubscriptions(transactionId: string) {
    return this.httpClient.delete(
      `${environment.appUrl}/api/v1/transaction/me/subscriptions/${transactionId}`
    );
  }

  getSubscriptions(accountId: string | null) {
    if (accountId === null) {
      return this.httpClient.get<Subscription[]>(
        `${environment.appUrl}/api/v1/transaction/me/subscriptions`
      );
    }
    return this.httpClient.get<Subscription[]>(
      `${environment.appUrl}/api/v1/transaction/me/subscriptions/${accountId}`
    );
  }

  editTransaction(transaction: UpdateTransaction) {
    return this.httpClient.put(
      `${environment.appUrl}/api/v1/transaction/me/edit`,
      transaction
    );
  }
}
