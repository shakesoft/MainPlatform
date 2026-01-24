import { Component, inject, OnInit } from '@angular/core';
import { OPACService } from '../services/o-pAC.service';

@Component({
  selector: 'lib-o-pAC',
  template: ` <p>o-pAC works!</p> `,
  styles: [],
})
export class OPACComponent implements OnInit {
  private service = inject(OPACService);

  ngOnInit(): void {
    this.service.sample().subscribe(console.log);
  }
}
