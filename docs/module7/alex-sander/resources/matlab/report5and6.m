h = [1 zeros(1,5) 0.9 zeros(1,5) 0.8];
H = fft(h);
f = [0:1/length(h):(length(h)-1)/length(h)]; %Using normalized frequencies
plot(f, abs(H), 'o');
xlabel('Frequency (Hz)');
ylabel('Amplitude');

hold on;

h = [1 zeros(1,5) 0.9 zeros(1,5) 0.8 zeros(1,39)];
H = fft(h);
f = [0:1/length(h):(length(h)-1)/length(h)];
plot(f, abs(H), '+')

hold on
plot(f, abs(H), 'r')