import { TestBed } from '@angular/core/testing';
import { PeculeTitleStrategy } from './pecule-title-strategy';
import { provideRouter, TitleStrategy } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { RouterTestingHarness } from '@angular/router/testing';
import { Component } from '@angular/core';

@Component({
  template: ''
})
class DummyPage {}

describe('PeculeTitleStrategy', () => {
  let service: PeculeTitleStrategy;
  let title: Title;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideRouter([
          {path: 'home', title: 'Accueil', component: DummyPage},
          {path: '', component: DummyPage},
        ]),
        { provide: TitleStrategy, useClass: PeculeTitleStrategy },
      ]
    });
    service = TestBed.inject(PeculeTitleStrategy);
    title = TestBed.inject(Title);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fall back to the app name when the route has no title', async () => {
    await RouterTestingHarness.create('/');
    const currentTitle = title.getTitle();
    expect(currentTitle).toBe('Pécule');
  })

  it('should render the title strategy', async () => {
    await RouterTestingHarness.create('/home');

    const currentTitle = title.getTitle();
    expect(currentTitle).toBe('Accueil – Pécule');
  })
});
