import {TransactionType} from "./enums/transactionType";
import {Category} from "./enums/category";

export interface AddTransaction {
  description: string,
  payee: string,
  type: TransactionType,
  value: number,
  category: Category,
  createdOnUtc: Date,
  accountId: string
}
