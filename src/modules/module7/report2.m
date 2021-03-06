function report2(a, savefiles)
close all;
N=44100*5;
N_imp=100;
%[x,Fs]=audioread('F:\Program Files (x86)\Steam Games\SteamApps\common\Age2HD\Sound\stream\open.mp3',[1,N]);
[x,Fs]=audioread('F:\Program Files (x86)\Steam Games\SteamApps\common\Age Of Empires 3\bin\Sound\music\interface\Noddinagushpa.mp3',[1,N]);
imp = [1 zeros(1,N_imp-1)];
x=x(1:end,1)';
T=1/Fs;
t=linspace(0,N*T,N);
t_imp=linspace(0,N_imp*T,N_imp);
%x=zeros(1,N);
%x(1)=1;
denominator = [1 a];
numerator = [1];
%X = fft(x);
y = filter(numerator,denominator,x);
h = filter(numerator,denominator,imp);
%Y = fft(y);
NFFT = 2^nextpow2(N); % Next power of 2 from length of y
NFFT_imp = 2^nextpow2(N_imp); % Next power of 2 from length of y
%NFFT = N;
%NFFT_imp = N_imp;
X = fft(x,NFFT)/N;
Y = fft(y,NFFT)/N;
H = fft(h,NFFT_imp)/N_imp;
IMP = fft(imp,NFFT_imp)/N_imp;
f = Fs/2*linspace(0,1,NFFT/2+1);
f_imp = Fs/2*linspace(0,1,NFFT_imp/2+1);
%Omega = [0: 2*pi/N : (N-1)*2*pi/N];
hold all;
set(gcf, 'Position', get(0,'Screensize')); % Maximize figure. 
% Impulse time plot
axisvalues = [0 max(t_imp) min([imp h 0]) max([imp h 0])];
%% Plot 1
subplot(4,2,1);
LinePlotReducer(t_imp,imp);
legend('impulse');
axis(axisvalues);
title('impulse(t)');
xlabel('t (s)');
ylabel('impulse');
%LinePlotReducer(handle1);

%% Plot 2
subplot(4,2,2);
LinePlotReducer(t_imp,h);
legend('h');
axis(axisvalues);
title('h(t)');
xlabel('t (s)');
ylabel('h');
%LinePlotReducer(handle2);

% Impulse freq plot
hplot = 2*abs(H(1:NFFT_imp/2+1));
impulseplot = 2*abs(IMP(1:NFFT_imp/2+1));
axisvalues = [0 max(f_imp) min([hplot impulseplot 0]) max([hplot impulseplot 0])];
%% Plot 3
subplot(4,2,3);
LinePlotReducer(f_imp,impulseplot);
legend('IMP');
axis(axisvalues);
title('Single-Sided Amplitude Spectrum of imp(t)');
xlabel('Frequency (Hz)');
ylabel('|IMPULSE(f)|');
%LinePlotReducer(handle3);

%% Plot 4
subplot(4,2,4);
LinePlotReducer(f_imp,hplot);
legend('H');
axis(axisvalues);
title('Single-Sided Amplitude Spectrum of h(t)');
xlabel('Frequency (Hz)');
ylabel('|H(f)|');
%LinePlotReducer(handle4);

% Audio time plot
axisvalues = [0 max(t) min([x y 0]) max([x y 0])];
%% Plot 5
subplot(4,2,5);
LinePlotReducer(t,x);
legend('x');
axis(axisvalues);
title('x(t)');
xlabel('t (s)');
ylabel('x');
%LinePlotReducer(handle5);

%% Plot 6
subplot(4,2,6);
LinePlotReducer(t,y);
legend('y');
axis(axisvalues);
title('y(t)');
xlabel('t (s)');
ylabel('y');
%LinePlotReducer(handle6);

% Plot values
xplot = 2*abs(X(1:NFFT/2+1));
yplot = 2*abs(Y(1:NFFT/2+1));
axisvalues = [0 max(f) min([xplot yplot 0]) max([xplot yplot 0])];
%% Plot 7
subplot(4,2,7);
LinePlotReducer(f,xplot);
legend('X');
axis(axisvalues);
title('Single-Sided Amplitude Spectrum of x(t)');
xlabel('Frequency (Hz)');
ylabel('|X(f)|');
%LinePlotReducer(handle7);

%% Plot 8
subplot(4,2,8);
LinePlotReducer(f,yplot);
legend('Y');
axis(axisvalues);
title('Single-Sided Amplitude Spectrum of y(t)');
xlabel('Frequency (Hz)');
ylabel('|Y(f)|');
%LinePlotReducer(handle8);

%Audio
audio = [x y];
audio = audio./max(abs(audio));
player = audioplayer(audio,Fs,24);
playblocking(player);
if savefiles
%saving
name=sprintf('a%0.2f',a);
saveas(gcf, strcat(name,'.fig'));
%cleanfigure;
matlab2tikz(strcat(name,'.tikz'), 'height', '\figureheight', 'width', '\figurewidth');
end;
end