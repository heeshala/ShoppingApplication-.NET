import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PaymentComponent } from './customer/payment/payment.component';
import { CustomerComponent } from './customer/customer.component';
import { ShopComponent } from './shop/shop.component'; 
import { DisplayProductsComponent } from './shop/display-products/display-products.component';
import { RegistrationComponent } from './customer/registration/registration.component';
import { EditCartComponent } from './customer/edit-cart/edit-cart.component';
import { LoginComponent } from './customer/login/login.component'; 
import { HistoryComponent } from './customer/history/history.component';
import { ViewProductComponent } from './shop/view-product/view-product.component';
import { AdminComponent } from './admin/admin.component';
import { AdminDashboardComponent } from './admin/admin-dashboard/admin-dashboard.component';
const routes: Routes = [
  {path:'shop',component:ShopComponent},
  {path:'profile',component:CustomerComponent},
  {path:'products',component:DisplayProductsComponent},
  {path:'registration',component:RegistrationComponent},
  {path:'cart',component:EditCartComponent},
  {path:'login',component:LoginComponent},
  {path:'orders',component:HistoryComponent},
  {path:'payment',component:PaymentComponent},
  {path:'item',component:ViewProductComponent},
  {path: "",  component: DisplayProductsComponent, pathMatch: "full"},
  {path: 'admin',  component: AdminComponent},
  {path: 'dashboard',  component: AdminDashboardComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
