import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';

import { FilterMenuModule, GridModule } from '@progress/kendo-angular-grid';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { ComboBoxModule } from '@progress/kendo-angular-dropdowns';
import { IndicatorsModule } from '@progress/kendo-angular-indicators';
import { SVGIconModule } from '@progress/kendo-angular-icons';
import { ToastrModule } from 'ngx-toastr';
import { DialogModule } from '@progress/kendo-angular-dialog';
import { PagerModule } from '@progress/kendo-angular-pager';
import { LabelModule } from '@progress/kendo-angular-label';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { FilterModule } from '@progress/kendo-angular-filter';

import { ConfigService } from './api/interceptor/configService';
import { serverUrlInterceptor } from './api/interceptor/serverUrlInterceptor';
import { FileSelectModule } from '@progress/kendo-angular-upload';
import { CommonModule } from '@angular/common';




@NgModule({
  declarations: [
    AppComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule, 
    AppRoutingModule, 
    GridModule,
    BrowserAnimationsModule, 
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule, // Add HttpClientModule here
    ButtonsModule,
    ComboBoxModule,
    IndicatorsModule,
    SVGIconModule, 
    DialogModule,
    PagerModule,
    InputsModule,
    DateInputsModule,
    LabelModule,
    FilterMenuModule,
    FilterModule,
    FileSelectModule,
    CommonModule,
    ToastrModule.forRoot({
      timeOut: 2000,
      positionClass: 'toast-top-right',
      preventDuplicates: true,
  })
  ],
  providers: [
    ConfigService,
    { provide: HTTP_INTERCEPTORS, useClass: serverUrlInterceptor, multi: true } // Register interceptor
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
