import { Component } from '@angular/core';
import { UserService } from '../user/user.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent {
  user$ = this.userService.user$;
  // private _transformer = (node: AccountNode, level: number) => ({
  //   expandable: !!node.children && node.children.length > 0,
  //   name: node.name,
  //   level: level,
  // });
  // treeControl = new FlatTreeControl<AccountNode>(
  //   (node) => node.level,
  //   (node) => node.expandable
  // );

  // treeFlattener = new MatTreeFlattener(
  //   this._transformer,
  //   (node) => node.level,
  //   (node) => node.expandable,
  //   (node) => node.children
  // );

  // dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);
  // hasChild = (_: number, node: AccountNode) =>
  //   !!node.children && node.children.length > 0;

  constructor(public userService: UserService) {}

  logout() {
    this.userService.logout();
  }
}
