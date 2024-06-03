import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AddTransaction } from '../shared/models/transaction/addTransaction';
import { environment } from '../../environments/environment.development';
import { Transaction } from '../shared/models/transaction/transaction';
import { UpdateTransaction } from '../shared/models/transaction/updateTransaction';

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

  editTransaction(transaction: UpdateTransaction) {
    return this.httpClient.put(
      `${environment.appUrl}/api/v1/transaction/me/edit`,
      transaction
    );
  }

  getSuggestedCategories(payee: string, description: string) {
    const message: string = `{
      "model": "gpt-3.5-turbo",
      "messages": [
          {
              "role": "user",
              "content": "Based on this Payee: ${payee}; and this description: ${description}; select one of the following categories that you think is the best suited: FoodAndDrinks, Shopping, House, Transportation, Vehicle, LifeAndEntertainment, FinancialExpenses, Investments, Income, Others? Answer only with the category!"
          }
      ]
  }`;
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${environment.openaiKey}`,
    });
    return this.httpClient.post(
      'https://api.openai.com/v1/chat/completions',
      message,
      { headers }
    );
  }
}
