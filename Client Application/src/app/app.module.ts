import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ShopComponent } from './shop/shop.component';
import { DisplayProductsComponent } from './shop/display-products/display-products.component';
import { CustomerComponent } from './customer/customer.component';

import { EditCartComponent } from './customer/edit-cart/edit-cart.component';
import { HistoryComponent } from './customer/history/history.component';
import { SharedService } from './shared.service';
import { AuthenticationService } from './services/authentication.service';
import { ShoppingService } from './services/shopping.service';
import { CustomerService } from './services/customer.service';

import { JwPaginationModule } from 'jw-angular-pagination';

import {HttpClientModule} from '@angular/common/http';
import {FormsModule,ReactiveFormsModule} from '@angular/forms';
import { RegistrationComponent } from './customer/registration/registration.component';
import { LoginComponent } from './customer/login/login.component';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { ViewProductComponent } from './shop/view-product/view-product.component';
import { PaymentComponent } from './customer/payment/payment.component';
import { AdminComponent } from './admin/admin.component';
import { AdminDashboardComponent } from './admin/admin-dashboard/admin-dashboard.component';

import { DataTablesModule } from 'angular-datatables';

@NgModule({
  declarations: [
    AppComponent,
    ShopComponent,
    DisplayProductsComponent,
    CustomerComponent,

    EditCartComponent,
    HistoryComponent,
    RegistrationComponent,
    LoginComponent,
    ViewProductComponent,
    PaymentComponent,
    AdminComponent,
    AdminDashboardComponent,
   
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    JwPaginationModule,
    NoopAnimationsModule,
    DataTablesModule

  ],
  providers: [SharedService,AuthenticationService,CustomerService,ShoppingService],
  bootstrap: [AppComponent]
})
export class AppModule { }
