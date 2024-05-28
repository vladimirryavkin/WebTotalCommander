import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from './configService';


@Injectable()
export class serverUrlInterceptor implements HttpInterceptor {

  constructor(private configService: ConfigService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
   
    const serverUrl = this.configService.getServerUrl();

    const modifiedRequest = request.clone({ url: serverUrl + request.url });

   
    return next.handle(modifiedRequest);
  }
}
