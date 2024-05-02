import {CurrencyEnum} from "../account/enums/currencyEnum";
import {TransactionType} from "./enums/transactionType";
import {Category} from "./enums/category";

export interface Transaction {
  value: number;
  currency: CurrencyEnum;
  accountValueBefore: number;
  accountValueAfter: number;
  description: string;
  payee: string;
  type: TransactionType;
  category: Category;
  accountId: string;
}
