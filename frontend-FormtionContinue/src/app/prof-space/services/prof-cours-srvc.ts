// src/app/prof-space/services/prof-cours-srvc.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ICours } from '../interfaces/icours';

export interface CourseCreateDto {
  titre: string;
  description: string;
  motsCles: string;
  categoryName: string;
}

export interface CourseUpdateDto {
  titre: string;
  description: string;
  motsCles: string;
  categoryName: string;
}

@Injectable({
  providedIn: 'root',
})
export class ProfCoursSrvc {
  constructor(private http: HttpClient) {}

  getCourses(search?: string, categoryId?: number): Observable<ICours[]> {
    const params: string[] = [];

    const s = search?.trim();
    if (s) params.push(`search=${encodeURIComponent(s)}`);

    if (categoryId !== undefined && categoryId !== null) params.push(`categoryId=${categoryId}`);

    const url =
      params.length > 0
        ? `${environment.apiUrl}/courses?${params.join('&')}`
        : `${environment.apiUrl}/courses`;

    return this.http.get<ICours[]>(url);
  }

  getById(id: number): Observable<ICours> {
    return this.http.get<ICours>(`${environment.apiUrl}/courses/${id}`);
  }

  create(payload: CourseCreateDto): Observable<ICours> {
    return this.http.post<ICours>(`${environment.apiUrl}/courses`, payload);
  }

  update(id: number, payload: CourseUpdateDto): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/courses/${id}`, payload);
  }

  attachPdf(id: number, file: File): Observable<void> {
    const fd = new FormData();
    fd.append('pdf', file);
    return this.http.post<void>(`${environment.apiUrl}/courses/${id}/attach-pdf`, fd);
  }

  
  publishCourse(id: number): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/courses/${id}/publish`, {});
  }

  unpublishCourse(id: number): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/courses/${id}/unpublish`, {});
  }

  deleteCourse(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/courses/${id}`);
  }

  getPdfBlob(id: number): Observable<Blob> {
    return this.http.get(`${environment.apiUrl}/courses/${id}/pdf`, { responseType: 'blob' });
  }

 


  // ---- Co-teachers ----
  getCoProfessors(courseId: number): Observable<any[]> {
    return this.http.get<any[]>(
      `${environment.apiUrl}/professor/courses/${courseId}/co-professors`
    );
  }

  searchProfessors(term: string): Observable<any[]> {
    return this.http.get<any[]>(
      `${environment.apiUrl}/professor/professors/search?term=${encodeURIComponent(term)}`
    );
  }

  addCoProfessor(courseId: number, profId: number): Observable<void> {
    return this.http.post<void>(
      `${environment.apiUrl}/professor/courses/${courseId}/co-professors/${profId}`,
      {}
    );
  }

  removeCoProfessor(courseId: number, profId: number): Observable<void> {
    return this.http.delete<void>(
      `${environment.apiUrl}/professor/courses/${courseId}/co-professors/${profId}`
    );
  }

  // ---- Enrollments ----
  getEnrollments(statut?: string, search?: string): Observable<any[]> {
    const params: string[] = [];

    const st = statut?.trim();
    if (st) params.push(`statut=${encodeURIComponent(st)}`);

    const s = search?.trim();
    if (s) params.push(`search=${encodeURIComponent(s)}`);

    const url =
      params.length > 0
        ? `${environment.apiUrl}/professor/enrollments?${params.join('&')}`
        : `${environment.apiUrl}/professor/enrollments`;

    return this.http.get<any[]>(url);
  }

  acceptEnrollment(id: number): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/professor/enrollments/${id}/accept`, {});
  }

  refuseEnrollment(id: number): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/professor/enrollments/${id}/refuse`, {});
  }
  attachVideo(id: number, file: File) {
    const fd = new FormData();
    fd.append('video', file);
    return this.http.post<void>(`${environment.apiUrl}/courses/${id}/attach-video`, fd);
  }
  
  getVideoBlob(id: number) {
    return this.http.get(`${environment.apiUrl}/courses/${id}/video`, {
      responseType: 'blob',
    });
  }
  
}
