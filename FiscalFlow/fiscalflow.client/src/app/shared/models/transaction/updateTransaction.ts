import { TransactionType } from './enums/transactionType';
import { Category } from './enums/category';

export interface UpdateTransaction {
  transactionId: string;
  description: string;
  payee: string;
  type: TransactionType;
  value: number;
  longitude: number | null;
  latitude: number | null;
  category: Category;
  createdOnUtc: Date;
}
