import { TransactionType } from './enums/transactionType';
import { Category } from './enums/category';

export interface AddTransaction {
  description: string;
  payee: string;
  type: TransactionType;
  value: number;
  category: Category;
  longitude: number | null;
  latitude: number | null;
  isRecursive: boolean;
  recurrence: number | null;
  imageUrl: string | null;
  createdOnUtc: Date;
  accountId: string;
}
