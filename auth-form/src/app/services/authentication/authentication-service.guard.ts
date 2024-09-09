import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthenticationService } from '../authentication.service';

export const authenticationServiceGuard: CanActivateFn = (route, state) => {
  const authenticationService = inject(AuthenticationService);
  return false;
};

// export class authenticationServiceGuard implements CanActivate {
//   constructor(private authService: AuthenticationService) { }

//   canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): MaybeAsync<GuardResult> {
//     return this.authService.checkAuthentication()
//   }
// }