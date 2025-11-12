import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ToastService } from '../services/toast.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private toast: ToastService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Ocorreu um erro inesperado';

        if (error.error?.error) {
          errorMessage = error.error.error;
        } else if (error.status === 0) {
          errorMessage = 'Sem conexão com internet. Verifique sua rede';
        } else if (error.status === 404) {
          errorMessage = 'Recurso não encontrado';
        } else if (error.status === 500) {
          errorMessage = 'Erro interno do servidor. Contate o suporte';
        } else if (error.status === 409) {
          errorMessage = error.error?.error || 'Recurso já existe';
        } else if (error.status === 400) {
          errorMessage = error.error?.error || 'Requisição inválida';
        }

        this.toast.error('Erro', errorMessage);
        return throwError(() => error);
      })
    );
  }
}
