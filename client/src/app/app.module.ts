import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

// My personal import
import { HttpClientModule } from '@angular/common/http';
// end
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
