export type UserRole = 'ADMIN' | 'PROFESSOR' | 'USER';

export interface Iuser {
  id: number;
  fullName: string;
  email: string;
  role: UserRole;
  createdAt: string;

  serviceId: number | null;
  serviceLibelle?: string;

  statutId: number | null;
  statutLibelle?: string;
}
