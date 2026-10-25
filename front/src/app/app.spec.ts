import { ComponentFixture, TestBed } from '@angular/core/testing';
import { App } from './app';
import { provideRouter } from '@angular/router';

// Everything that can be focusable with keyboard
const FOCUSABLE =
  'a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), ' +
  'textarea:not([disabled]), [tabindex]:not([tabindex="-1"])';

describe('App', () => {
  let fixture: ComponentFixture<App>;
  let compiled: HTMLElement;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideRouter([])],
    })
      .compileComponents();
      fixture = TestBed.createComponent(App);
      fixture.detectChanges();
      compiled = fixture.nativeElement as HTMLElement;
  });

  it('should create the app', () => {
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should make the skip link the first focusable element', () => {
    const firstFocusable = compiled.querySelector(FOCUSABLE);
    expect(firstFocusable?.classList.contains('skip-link')).toBe(true);
  });

  it('should target a focusable main region', () => {
    const href = compiled.querySelector('a.skip-link')?.getAttribute('href');
    expect(href).toMatch(/^#.+/); // Ensure to be an anchor

    const target = href ? compiled.querySelector(href) : null;
    expect(target?.tagName).toBe('MAIN');
    expect(target?.getAttribute('tabindex')).toBe('-1');
  });
});
