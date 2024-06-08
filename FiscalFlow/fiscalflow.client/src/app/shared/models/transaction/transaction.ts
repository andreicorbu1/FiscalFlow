import { CurrencyEnum } from '../account/enums/currencyEnum';
import { TransactionType } from './enums/transactionType';
import { Category } from './enums/category';

export interface Transaction {
  id: string;
  value: number;
  currency: CurrencyEnum;
  accountValueBefore: number;
  accountValueAfter: number;
  description: string;
  payee: string;
  type: TransactionType;
  imageUrl: string | null;
  longitude: number;
  latitude: number;
  category: Category;
  createdOnUtc: Date;
  accountId: string;
}
