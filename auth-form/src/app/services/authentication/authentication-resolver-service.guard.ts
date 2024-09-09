import { ActivatedRouteSnapshot, MaybeAsync, Resolve, RouterStateSnapshot } from "@angular/router";

export class authenticationResolverServiceGuard<T> implements Resolve<T> {
  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): MaybeAsync<T> {
    throw new Error('Method not implemented.');
  }
}