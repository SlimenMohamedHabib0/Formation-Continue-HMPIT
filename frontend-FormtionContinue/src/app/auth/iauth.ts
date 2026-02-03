export type Role = 'ADMIN' | 'PROFESSOR' | 'USER';

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  fullName: string;
  email: string;
  password: string;
  serviceId: number;
  statutId: number;
}

export interface AuthResponseDto {
  userId: string;
  fullName: string;
  email: string;
  role: Role;
  serviceId: number;
  serviceLibelle: string;
  statutId: number;
  statutLibelle: string;
  token: string;
}

export interface MeResponse {
  id: string;
  fullName: string;
  email: string;
  role: Role;
  serviceId: number;
  serviceLibelle: string;
  statutId: number;
  statutLibelle: string;
}
