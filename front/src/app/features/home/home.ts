import { httpResource } from '@angular/common/http';
import { Component, computed } from '@angular/core';

interface Status {
  application: string;
  version: string;
}

@Component({
  imports: [],
  selector: 'app-home',
  styleUrl: './home.scss',
  templateUrl: './home.html',
})
export class Home {
  protected readonly status = httpResource<Status>(() => '/api/status');
  protected readonly version = computed(() => this.status.value()?.version ?? 'inconnue');
}
