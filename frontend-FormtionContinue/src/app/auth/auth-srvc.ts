import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthResponseDto, LoginDto, MeResponse, RegisterDto } from './iauth';

@Injectable({
  providedIn: 'root',
})
export class AuthSrvc {
  private readonly TOKEN_KEY = 'access_token';
  private readonly ME_KEY = 'auth_me';
  private readonly baseUrl = `${environment.apiUrl}/Auth`;

  private readonly meSubject = new BehaviorSubject<MeResponse | null>(null);
  readonly me$ = this.meSubject.asObservable();

  constructor(private http: HttpClient) {
    const saved = localStorage.getItem(this.ME_KEY);
    if (saved) {
      try {
        this.meSubject.next(JSON.parse(saved) as MeResponse);
      } catch {
        localStorage.removeItem(this.ME_KEY);
      }
    }
  }

  get token(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  private setMe(me: MeResponse | null): void {
    this.meSubject.next(me);
    if (me) localStorage.setItem(this.ME_KEY, JSON.stringify(me));
    else localStorage.removeItem(this.ME_KEY);
  }

  clearToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.setMe(null);
  }

  get meSnapshot(): MeResponse | null {
    return this.meSubject.value;
  }

  get isLoggedIn(): boolean {
    return !!this.token;
  }

  login(dto: LoginDto): Observable<AuthResponseDto> {
    return this.http
      .post<AuthResponseDto>(`${this.baseUrl}/login`, dto)
      .pipe(
        tap((res) => {
          this.setToken(res.token);
          this.setMe({
            id: res.userId,
            fullName: res.fullName,
            email: res.email,
            role: res.role,
            serviceId: res.serviceId,
            serviceLibelle: res.serviceLibelle,
            statutId: res.statutId,
            statutLibelle: res.statutLibelle,
          });
        })
      );
  }

  register(dto: RegisterDto): Observable<string> {
    return this.http.post(`${this.baseUrl}/register`, dto, {
      responseType: 'text',
    });
  }

  fetchMe(): Observable<MeResponse> {
    return this.http
      .get<MeResponse>(`${this.baseUrl}/me`)
      .pipe(tap((me) => this.setMe(me)));
  }

  logout(): void {
    this.clearToken();
  }
}
