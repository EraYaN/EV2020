
%% Time domain and frequency domain
clear all
close all
clc;
load train.mat
p=audioplayer(y,Fs);
play(p)
get(p)

%Plotten van het signaal in het tijdsdomein
figure;
t=[0:(length(y)/Fs)/(length(y)-1):(length(y)/Fs)];
plot(t,y)
xlabel('time [s]');

%Fouriertransformatie
Y=fft(y);
omega=[0: 2*pi/length(y) : (length(y)-1)*2*pi/length(y)]-pi;
figure;
plot(omega,abs(Y))
xlabel('f [Hz]');

%% Time frequency plot
% Matrix X aanmaken
% 164 rijen, 79 kolommen
kolom=ceil(length(y)/Fs /0.02);
rij=ceil(length(y)/(length(y)/Fs /0.02));
X=zeros(rij,kolom);
signaal=[y ;zeros(76,1)];
X= reshape(signaal,rij,kolom);

Y=fft(X);
t=[0:0.02 :(length(y)/Fs)];
f=[0: 2*pi/rij : (rij-1)*2*pi/rij]-pi;
xlabel('time [s]');
ylabel('f [Hz]');
figure;
imagesc(t,f,abs(Y))
