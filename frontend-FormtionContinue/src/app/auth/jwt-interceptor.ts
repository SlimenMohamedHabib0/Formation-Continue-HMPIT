import { Injectable } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { AuthSrvc } from './auth-srvc';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private auth: AuthSrvc, private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.auth.token;

    const isApiCall = req.url.startsWith(environment.apiUrl);
    const authReq =
      token && isApiCall
        ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) // clone request and attach token to header only for API calls and non null tokens
        : req;

    return next.handle(authReq).pipe(
      catchError((err: unknown) => {
        if (err instanceof HttpErrorResponse) {
          if (err.status === 401) { // if not logged in or token expired
            this.auth.clearToken();
            this.router.navigate(['/auth/login']);
          }
          if (err.status === 403) {
            const url = this.router.url || '';
            if (url.startsWith('/admin') || url.startsWith('/prof')) {
              this.router.navigate(['/forbidden']);
            }
          }
          
        }
        return throwError(() => err);
      })
    );
  }
}
