import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class PublicSrvc {
  constructor(private http: HttpClient) {}

  getUserResultsSummary() {
    return this.http.get<any>(`${environment.apiUrl}/user/results/summary`);
  }

  getMyAttempts(search?: string, courseId?: number) {
    const params: string[] = [];
    const s = search?.trim();
    if (s) params.push(`search=${encodeURIComponent(s)}`);
    if (courseId !== undefined && courseId !== null) params.push(`courseId=${courseId}`);

    const url =
      params.length > 0
        ? `${environment.apiUrl}/user/attempts?${params.join('&')}`
        : `${environment.apiUrl}/user/attempts`;

    return this.http.get<any[]>(url);
  }

  getPublishedCourses(search?: string) {
    const params: string[] = [];
    const s = search?.trim();
    if (s) params.push(`search=${encodeURIComponent(s)}`);

    const url =
      params.length > 0
        ? `${environment.apiUrl}/user-courses?${params.join('&')}`
        : `${environment.apiUrl}/user-courses`;

    return this.http.get<any[]>(url);
  }

  requestEnrollment(courseId: number) {
    return this.http.post<void>(`${environment.apiUrl}/courses/${courseId}/enrollments/request`, {});
  }

  getMyEnrollments(search?: string, statut?: string) {
    const params: string[] = [];
    const s = search?.trim();
    if (s) params.push(`search=${encodeURIComponent(s)}`);
    const st = statut?.trim();
    if (st) params.push(`statut=${encodeURIComponent(st)}`);

    const url =
      params.length > 0
        ? `${environment.apiUrl}/my/enrollments?${params.join('&')}`
        : `${environment.apiUrl}/my/enrollments`;

    return this.http.get<any[]>(url);
  }

  getCourseById(courseId: number) {
    return this.http.get<any>(`${environment.apiUrl}/user-courses/${courseId}`);
  }

  getCoursePdfBlob(courseId: number) {
    return this.http.get(`${environment.apiUrl}/user-courses/${courseId}/pdf`, {
      responseType: 'blob',
    });
  }
  getCourseVideoUrl(courseId: number): string {
    return `${environment.apiUrl}/user-courses/${courseId}/video`;
  }
  

  getCourseVideoBlob(courseId: number) {
    return this.http.get(`${environment.apiUrl}/user-courses/${courseId}/video`, {
      responseType: 'blob',
    });
  }

  getProgress(courseId: number) {
    return this.http.get<any>(`${environment.apiUrl}/courses/${courseId}/progress`);
  }

  updateProgress(courseId: number, page: number, totalPages: number) {
    return this.http.post<void>(
      `${environment.apiUrl}/courses/${courseId}/progress?page=${page}&totalPages=${totalPages}`,
      {}
    );
  }

  getQcm(courseId: number) {
    return this.http.get<any>(`${environment.apiUrl}/user-courses/${courseId}/qcm`);
  }

  submitQcm(courseId: number, payload: any) {
    return this.http.post<any>(`${environment.apiUrl}/user-courses/${courseId}/qcm/submit`, payload);
  }
}
