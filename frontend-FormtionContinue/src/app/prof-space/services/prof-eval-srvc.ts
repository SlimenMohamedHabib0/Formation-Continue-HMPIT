import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs'
import { environment } from '../../../environments/environment'
import { IQcmValidityDto, Iquestion, IquestionCreateDto, IquestionUpdateDto } from '../interfaces/iquestion'

@Injectable({
  providedIn: 'root',
})
export class ProfEvalSrvc {
  constructor(private http: HttpClient) {}

  getQuestions(courseId: number): Observable<Iquestion[]> {
    return this.http.get<Iquestion[]>(`${environment.apiUrl}/courses/${courseId}/questions`)
  }

  addQuestion(courseId: number, payload: IquestionCreateDto): Observable<Iquestion> {
    return this.http.post<Iquestion>(`${environment.apiUrl}/courses/${courseId}/questions`, payload)
  }

  updateQuestion(courseId: number, questionId: number, payload: IquestionUpdateDto): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/courses/${courseId}/questions/${questionId}`, payload)
  }

  deleteQuestion(courseId: number, questionId: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/courses/${courseId}/questions/${questionId}`)
  }

  getQcmValidity(courseId: number): Observable<IQcmValidityDto> {
    return this.http.get<IQcmValidityDto>(`${environment.apiUrl}/courses/${courseId}/qcm-validity`)
  }

  publishQcm(courseId: number): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/courses/${courseId}/qcm/publish`, {})
  }
}
