import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Iprofesseur } from './interfaces/iprofesseur';
import { Iuser } from './interfaces/iuser';
import { Icours } from './interfaces/icours';
import { Icategory } from './interfaces/icategory';
import { Ienrollment } from './interfaces/ienrollment';
import { Iattempt } from './interfaces/iattempt';

export interface CountItemDto {
  id: number;
  label: string;
  count: number;
}

export interface AdminDashboardDto {
  nbUsers: number;
  nbProfessors: number;
  nbAdmins: number;

  nbCategories: number;
  nbCoursesTotal: number;
  nbCoursesDraft: number;
  nbCoursesPublished: number;

  nbEnrollmentsTotal: number;
  nbEnrollmentsPending: number;
  nbEnrollmentsAccepted: number;
  nbEnrollmentsRefused: number;

  nbAttemptsTotal: number;
  nbAttemptsPassed: number;
  nbAttemptsFailed: number;
  averageNote: number | null;
  successRatePercent: number;

  topCoursesByEnrollments: CountItemDto[];
  topCategoriesByCourses: CountItemDto[];
  topCategoriesByEnrollments: CountItemDto[];
}

export interface CreateProfessorDto {
  fullName?: string;
  email?: string;
  password?: string;

  [key: string]: any;
}

export interface UpdateProfessorDto {
  fullName?: string;
  email?: string;
  password?: string;

  [key: string]: any;
}

export interface UpdateUserDto {
  fullName?: string;
  email?: string;
  role?: 'ADMIN' | 'PROFESSOR' | 'USER';
  password?: string;

  [key: string]: any;
}

@Injectable({
  providedIn: 'root',
})
export class AdminSrvc {
  constructor(private http: HttpClient) {}

  getDashboard(): Observable<AdminDashboardDto> {
    return this.http.get<AdminDashboardDto>(`${environment.apiUrl}/admin/dashboard`);
  }

  getUsers(search?: string, role?: string): Observable<Iuser[]> {
    const params: string[] = [];

    const s = search?.trim();
    if (s) params.push(`search=${encodeURIComponent(s)}`);

    const r = role?.trim();
    if (r) params.push(`role=${encodeURIComponent(r)}`);

    const url =
      params.length > 0
        ? `${environment.apiUrl}/admin/users?${params.join('&')}`
        : `${environment.apiUrl}/admin/users`;

    return this.http.get<Iuser[]>(url);
  }

  getUserById(id: number): Observable<Iuser> {
    return this.http.get<Iuser>(`${environment.apiUrl}/admin/users/${id}`);
  }

  updateUser(id: number, payload: UpdateUserDto): Observable<Iuser> {
    return this.http.put<Iuser>(`${environment.apiUrl}/admin/users/${id}`, payload);
  }

  getProfessors(search?: string): Observable<Iprofesseur[]> {
    const url = search
      ? `${environment.apiUrl}/admin/professors?search=${encodeURIComponent(search)}`
      : `${environment.apiUrl}/admin/professors`;

    return this.http.get<Iprofesseur[]>(url);
  }

  createProfessor(payload: CreateProfessorDto): Observable<Iprofesseur> {
    return this.http.post<Iprofesseur>(`${environment.apiUrl}/admin/professors`, payload);
  }

  updateProfessor(id: number, payload: UpdateProfessorDto): Observable<Iprofesseur> {
    return this.http.put<Iprofesseur>(`${environment.apiUrl}/admin/professors/${id}`, payload);
  }

  deleteProfessor(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/admin/professors/${id}`);
  }

  getCourses(
    search?: string,
    categoryId?: number,
    etat?: 'PUBLISHED' | 'DRAFT'
  ): Observable<Icours[]> {
    const params: string[] = [];

    const s = search?.trim();
    if (s) params.push(`search=${encodeURIComponent(s)}`);

    if (categoryId !== undefined && categoryId !== null) params.push(`categoryId=${categoryId}`);

    if (etat) params.push(`etat=${encodeURIComponent(etat)}`);

    const url =
      params.length > 0
        ? `${environment.apiUrl}/admin/courses?${params.join('&')}`
        : `${environment.apiUrl}/admin/courses`;

    return this.http.get<Icours[]>(url);
  }

  unpublishCourse(id: number): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/admin/courses/${id}/unpublish`, {});
  }

  deleteDraftCourse(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/admin/courses/${id}`);
  }
  
  getEnrollments(
    search?: string,
    statut?: string,
    courseId?: number,
    categoryId?: number
  ) {
    const params: string[] = [];
  
    const s = search?.trim();
    if (s) params.push(`search=${encodeURIComponent(s)}`);
  
    const st = statut?.trim();
    if (st) params.push(`statut=${encodeURIComponent(st)}`);
  
    if (courseId !== undefined && courseId !== null)
      params.push(`courseId=${courseId}`);
  
    if (categoryId !== undefined && categoryId !== null)
      params.push(`categoryId=${categoryId}`);
  
    const url =
      params.length > 0
        ? `${environment.apiUrl}/admin/enrollments?${params.join('&')}`
        : `${environment.apiUrl}/admin/enrollments`;
  
    return this.http.get<Ienrollment[]>(url);
  }

  getAttempts(
    search?: string,
    statut?: string,
    courseId?: number,
    categoryId?: number
  ) {
    const params: string[] = [];
  
    const s = search?.trim();
    if (s) params.push(`search=${encodeURIComponent(s)}`); // changes special characters in search
  
    const st = statut?.trim();
    if (st) params.push(`statut=${encodeURIComponent(st)}`);
  
    if (courseId !== undefined && courseId !== null)
      params.push(`courseId=${courseId}`);
  
    if (categoryId !== undefined && categoryId !== null)
      params.push(`categoryId=${categoryId}`);
  
    const url =
      params.length > 0
        ? `${environment.apiUrl}/admin/attempts?${params.join('&')}`
        : `${environment.apiUrl}/admin/attempts`;
  
    return this.http.get<Iattempt[]>(url);
  }
  getCategories(search?: string) {
    const s = search?.trim();
    const url = s
      ? `${environment.apiUrl}/categories?search=${encodeURIComponent(s)}`
      : `${environment.apiUrl}/categories`;
  
    return this.http.get<Icategory[]>(url);
  }
  
  updateCategory(id: number, payload: { libelle: string }) {
    return this.http.put<void>(
      `${environment.apiUrl}/categories/${id}`,
      payload
    );
  }
  
  deleteCategory(id: number) {
    return this.http.delete<void>(
      `${environment.apiUrl}/categories/${id}`
    );
  }
  
  
}
