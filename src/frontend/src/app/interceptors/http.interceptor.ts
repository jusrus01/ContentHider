import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
  HttpResponse,
} from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { catchError, switchMap, tap } from 'rxjs/operators';
import { RoutingService } from '../services/routing.service';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { ErrorSnackComponent } from '../common/snacks/error/error.snack';
import { SuccessSnackComponent } from '../common/snacks/error/success.snack';
import { LoginResource } from '../resources/login.resource';

@Injectable()
export class HttpClientInterceptor implements HttpInterceptor {
  private readonly UNAUTHORIZED = 401;

  private readonly FORBIDDEN = 403;
  private readonly METHOD_NOT_ALLOWED = 405;

  private readonly NOTFOUND = 404;

  constructor(
    private authService: AuthService,
    private routing: RoutingService,
    private loginResource: LoginResource,
    private snackBar: MatSnackBar
  ) {}

  private handleAuthError(
    err: HttpErrorResponse,
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<any> {
    const status = err.status;
    if (status === this.UNAUTHORIZED) {
      // Use RxJS operators instead of async/await

      const creds = this.authService.getCreds();
      if (creds === null) {
        return of(err.message);
      }

      return this.loginResource.refresh(creds).pipe(
        switchMap((creds: any) => {
          if (creds === null || creds === undefined) {
            return throwError(() => err);
          }
          return this.loginResource.refresh(creds).pipe(
            switchMap((refreshResponse) => {
              if (refreshResponse === null) {
                return throwError(() => err);
              }
              this.authService.setCreds(refreshResponse);
              console.log('credentials refreshed successfully');

              return next.handle(
                request.clone({
                  headers: request.headers.set(
                    'Authorization',
                    `Bearer ${refreshResponse.token}`
                  ),
                  withCredentials: true,
                })
              );
            })
          );
        }),
        catchError(() => throwError(() => err))
      );
    }

    if (status == this.METHOD_NOT_ALLOWED || status == this.FORBIDDEN) {
      this.authService.logout();
      this.routing.goToLogin();
      return of(err.message);
    }

    if (status == this.NOTFOUND) {
      this.routing.goToNotFound();
      return of(err.message);
    }

    this.snackBar.openFromComponent(ErrorSnackComponent, {
      duration: 1500,
      data: err.error.message,
      panelClass: ['red-snackbar-error'],
    } as MatSnackBarConfig);
    return of(err.message);
  }

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    let requestHeaders = request.headers;
    const creds = this.authService.getCreds();
    if (creds !== null) {
      requestHeaders = request.headers.set(
        'Authorization',
        `Bearer ${creds.token}`
      );
    }

    if (request.method === 'OPTIONS') {
      return next
        .handle(
          request.clone({
            headers: requestHeaders,
            withCredentials: true,
          })
        )
        .pipe(catchError((err) => this.handleAuthError(err, request, next)));
    }

    return next
      .handle(
        request.clone({
          headers: requestHeaders,
          withCredentials: true,
        })
      )
      .pipe(
        tap((event) => {
          if (
            event instanceof HttpResponse &&
            !request.url.endsWith('refresh-login') &&
            (request.method === 'POST' || request.method === 'PUT')
          ) {
            this.snackBar.openFromComponent(SuccessSnackComponent, {
              duration: 1000,
              panelClass: ['red-snackbar'],
            });
          }
        }),
        catchError((err) => this.handleAuthError(err, request, next))
      );
  }
}
