import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_service/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any; // To pass down parameters from the parent.
  @Output() cancelRegister = new EventEmitter(); // Emit an event for the parent to capture and act upon it.
  model: any = {};
  constructor(private authService: AuthService) { }
  ngOnInit() {}
  register() {
    this.authService.register(this.model).subscribe(() => {
      console.log('registration ok');
    }, error => {
      console.log('error registering!', error);
    });
  }
  cancel() {
    this.cancelRegister.emit(false);
    console.log('cancelled');
  }
}
