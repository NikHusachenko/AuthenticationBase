import { Component } from '@angular/core';
import { AuthenticationService } from '../../../services/authentication.service';
import { FormsModule } from '@angular/forms';

class LoginModel {
  fullName?: string;
  login?: string;
  password?: string;
}

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css'
})

export class SignUpComponent {
  protected loginModel: LoginModel = new LoginModel();

  constructor(private authenticationService: AuthenticationService) { }

  protected async signUp() {
    if (!this.loginModel.fullName ||
        !this.loginModel.fullName ||
        !this.loginModel.password) {
          return;
        }

    let response = await this.authenticationService.signUp(
      this.loginModel.fullName!, 
      this.loginModel.login!, 
      this.loginModel.password!);
  }
}