import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_service/auth.service';
import { AlertifyService } from '../_service/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any; // To pass down parameters from the parent.
  @Output() cancelRegister = new EventEmitter(); // Emit an event for the parent to capture and act upon it.
  model: any = {};
  constructor(private authService: AuthService, private alertify: AlertifyService) { }
  ngOnInit() {}
  register() {
    this.authService.register(this.model).subscribe(() => {
      // console.log('registration ok');
      this.alertify.success('Registration Succesfull');
    }, error => {
      // console.log('error registering!', error);
      this.alertify.error('Error registering');
    });
  }
  cancel() {
    this.cancelRegister.emit(false);
    // console.log('cancelled');
  }
}
