import { Component, OnInit ,OnDestroy, Output, EventEmitter } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { HttpClient, HttpEventType, HttpErrorResponse } from '@angular/common/http';

import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';

import Swal from 'sweetalert2';
import { AdminService } from 'src/app/services/admin.service';
import { ShoppingService } from 'src/app/services/shopping.service';

import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit,OnDestroy  {


  products:any=[];
  sellers:any=[];

  id:any;
  token:any;
  dtOptions: DataTables.Settings = {};
  dtTrigger: Subject<void> = new Subject<void>();
  numberOnly = "^0|[0-9]\d*";
  //Create FormGroup
requiredForm: FormGroup= this.fb.group({
  price: ['', Validators.compose([Validators.required,Validators.min(1),Validators.pattern(this.numberOnly)])],
  category: ['', Validators.required],
  productname: ['', Validators.required],
  quantity: ['', Validators.compose([Validators.required,Validators.min(1),Validators.pattern(this.numberOnly)])],
 description: ['', Validators.required],
 selectedFiles:['', Validators.required],
});

productname:any;
quantity:any;
category:any;
price:any;
seller:any;
description:any;
sellerId:any;
sellerName:any;

fileSelected:any;


selectedFiles?: FileList;
currentFile?: File;
progress = 0;
message = '';
fileInfos?: Observable<any>;
CurrentQuantity:any;

productById:any=[];

  constructor(private http: HttpClient,private shopping:ShoppingService,private service:AdminService,private fb: FormBuilder,private cookieService: CookieService,private router: Router) { }



  
  ngOnInit(): void {
    
    if(this.cookieService.check('token')){
      if(this.cookieService.get('role')=="admin"){
        this.products=[];
        console.log(this.products);
        this.token=this.cookieService.get('token');
      this.id=this.cookieService.get('adminId');
        this.getAllProducts();
        this.getSellers();
        
      }else{
        console.log("nocookie")
      this.router.navigate(["admin"]);
      }
      
      
    }else{
      console.log("nocookie")
      this.router.navigate(["login"]);
    }
  }


  //File Uploading
  selectFile(event: any): void {
    this.selectedFiles = event.target.files;
    if(this.selectedFiles!=null){
      this.fileSelected="Yes"
    }else{
      this.fileSelected="";
    }
    
  }
postId:any;
imageSuccess:any;
  upload(prodId:any): void {
    this.progress = 0;
    if (this.selectedFiles) {
      const file: File | null = this.selectedFiles.item(0);
      if (file) {
        this.currentFile = file;
        console.log(this.currentFile);
        this.service.upload(this.currentFile,this.token,this.productId).subscribe(data=>{
          this.imageSuccess=data;
          console.error(this.imageSuccess);
              if(this.imageSuccess==0){
                Swal.fire({
                  position: 'center',
                  icon: 'warning',
                  title: 'Product Registered \nUnable To Save Image',
                  showConfirmButton: false,
                  timer: 1500
                }).then(() => {
                  window.location.reload();
                });
                
              }else{
                Swal.fire({
                  position: 'center',
                  icon: 'success',
                  title: 'Product Registration Successfull',
                  showConfirmButton: false,
                  timer: 1500
                }).then(() => {
                  window.location.reload();
                });
              }
         });
      }
      this.selectedFiles = undefined;
    }
  }

  //

  getAllProducts(){
    
    this.service.getProducts(this.token).subscribe(data=>{
      this.products=data;
      this.dtTrigger.next();
    });

    
  }

  setSeller(id:any,name:any){
     this.sellerId=id;
     this.sellerName=name;
  }


  getSellers(){
    this.service.getAllSellers(this.token).subscribe(data=>{
      this.sellers=data;
      this.sellerId=this.sellers[0].SellerId;
      this.sellerName=this.sellers[0].SellerName;
    });

    
  }


  productId:any;
  addProduct(){
    const body={ProductName:this.productname,SellerId:this.sellerId,Quantity:this.quantity,Category:this.category,Description:this.description,Price:this.price};
    this.service.addProduct(body,this.token).subscribe(data=>{
      this.productId=data;
      if(this.productId>0){
        this.upload(this.productId);
      }else{
        Swal.fire({
          position: 'center',
          icon: 'warning',
          title: 'Unable To Register Product',
          showConfirmButton: false,
          timer: 1500
        }).then(() => {
          window.location.reload();
        });
      }
      
    });
  }

  //Get Edit Info
  getEditInfo(id:any){
    this.shopping.getProductById(id).subscribe(data=>{
      this.productById=data;
      this.CurrentQuantity=this.productById[0].Quantity;
      
    });
  }

  decrement(){
    if(this.CurrentQuantity!=0){
      this.CurrentQuantity--;
    }
  }

  increment(){
    this.CurrentQuantity++;
  }

  updateStocks(id:any,newquantity:any){
    const body={ProductId:id,Quantity:newquantity}
    this.service.updateStock(body,this.token).subscribe(data=>{
      this.productId=data;
      if(this.productId==1){
        Swal.fire({
          position: 'center',
          icon: 'success',
          title: 'Product Updated Successfully',
          showConfirmButton: false,
          timer: 1500
        }).then(() => {
          window.location.reload();
        });
      }else{
        Swal.fire({
          position: 'center',
          icon: 'warning',
          title: 'Unable To Update Product',
          showConfirmButton: false,
          timer: 1500
        }).then(() => {
          window.location.reload();
        });
      }
      
    });
  }

  ngOnDestroy(): void {
    this.dtTrigger.unsubscribe();
  }

  logout(){
    this.cookieService.delete('adminId');
       this.cookieService.delete('adminName');
       this.cookieService.delete('role');
       this.cookieService.delete('token');
       this.router.navigate(["login"]).then(() => {
        window.location.reload();
      });
      
  }

}
