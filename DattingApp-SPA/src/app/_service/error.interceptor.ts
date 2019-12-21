import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(
    req: import('@angular/common/http').HttpRequest<any>,
    next: import('@angular/common/http').HttpHandler
  ): import('rxjs').Observable<import('@angular/common/http').HttpEvent<any>> {
    return next.handle(req).pipe(
        catchError(error => {
            if (error.status === 401) {
                return throwError(error.statusText);
            }
            if (error instanceof HttpErrorResponse) {
                const appError = error.headers.get('Application-Error');
                if (appError) {
                    return throwError(appError);
                }
                const svrError = error.error;
                let modalStateErrors = '';
                if (svrError.error &&
                    typeof svrError.errors === 'object') {
                        for (const k in svrError.errors) {
                            if (svrError.errors[k]) {
                                modalStateErrors += svrError.errors[k] + '\n';
                            }
                        }
                }
                return throwError(modalStateErrors || svrError || 'Server Error');;
            }
        })
    );
  }
}

export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
};
