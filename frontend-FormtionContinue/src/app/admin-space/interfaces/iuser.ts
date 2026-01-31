export interface Iuser {
    id: number;
    fullName: string;
    email: string;
    role: 'ADMIN' | 'PROFESSOR' | 'USER';
    createdAt: string;
  }
  